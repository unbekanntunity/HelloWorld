import React, { Component } from 'react';
import Post from '../components/Post';
import { Link, useNavigate } from 'react-router-dom';
import profile from '../images/profile.png';

import './Home.css';

import { Dialog, ReportDialog } from '../components/Dialog';
import InputField from '../components/InputField/InputField';

import RankingBox from '../components/RankingBox';

import { sendJSONRequest } from '../requestFuncs';

class Home extends Component {
    state = {
        posts: {
            
        },
        topDiscussion: [
            {
                name: "Hello",
                tags: [
                    "C#",
                    "F#",
                ]
            }
        ],
        topProjects: [
            {
                name: "Hello World",
                tags: [
                    "C#",
                    "Social",
                ]
            }
        ],
        inputFieldDefaults: [
            {
                icon: profile,
                header: 'MonarchSoftworks',
                text: 'You: Hello there',
                sideNote: '11.22'
            },
            {
                icon: profile,
                header: 'MonarchSoftworks',
                text: 'You: Hello there',
                sideNote: '11.22'
            },
            {
                icon: profile,
                header: 'MonarchSoftworks',
                text: 'You: Hello there',
                sideNote: '11.22'
            },
        ],
       
        showShareDialog: false,
        showReportDialog: false,
        mulitInputText: 'short description'
    }

    componentDidMount() {
        this.getPosts();
    }

    getPosts = () => {
        sendJSONRequest("GET", "/post/get_all", undefined, this.props.tokens.token)
            .then(response => {
            this.setState({ posts: response.data })
        }, error => {
            console.log(error);
            this.props.onError(error.message)
        });
    }

    handleCreatorInfos = (index) => {
        let newPosts = this.state.posts;

        sendJSONRequest("GET", `/users/get/${this.state.posts[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {
                newPosts[index].creatorImage = response.data.image;
                newPosts[index].creatorName = response.data.userName;
                this.setState({ posts: newPosts })
            }, error => {
                console.log(error);
                this.props.onError(error.message);
            });
    }

    handleOnNotificationIconClick = () => {

    }

    handlePageSwitch = (page) => {
        const navigate = useNavigate();
        navigate(page)
    }

    handleReportShow = () => {
        this.setState({ showReportDialog: true });
    }

    handleReportSent = () => {
        this.setState({ showReportSentDialog: !this.state.showReportSentDialog });
    }

    handleShare = () => {
         this.setState({ showShareDialog: !this.state.showShareDialog });
    }

    handleMulitLineChange = (event) => {
        this.setState({ mulitInputText: event.target.value });
        console.log(this.state.mulitInputText);
    }

    handleReasonSelected = (reason) => {
        this.setState({ showReportSentDialog: !this.state.showReportSentDialog });
    }

    render() {
        return (
            <div style={{height: "100%"} }>
                <div className="home-body">
                    <div className="footer-box">
                        <Link className="link" to="/impressum"><p>Impressum</p></Link >
                        <Link className="link" to="/policies"><p>Policies</p></Link >
                        <Link className="link" to="/support"><p>Support</p></Link >
                    </div>
                    <div className="home-posts">
                        {
                            Array.from(this.state.posts).map((post, index) =>
                                <Post key={index} keyProp={index} text={post.content} title={post.title} images={post.imageUrls} imageHeight={200}
                                    onFirstAppear={this.handleCreatorInfos} onReportClick={() => this.setState({ showReportDialog: true })} width={400} />
                            )
                        }
                    </div>
                    <div>
                        <div className="ranking-box-wrapper">
                            <RankingBox width="200px" height="250px" title="Top discussion" items={this.state.topDiscussion} />
                        </div>
                        <div className="ranking-box-wrapper">
                            <RankingBox width="200px" height="250px" title="Top discussion" items={this.state.topProjects} />
                        </div>
                    </div>
                </div>
                {
                    this.state.showShareDialog && 
                    <Dialog title="Share" onBackClick={this.handleShare} paddingX="10px" paddingY="20px"
                            height="fit-content" width="400px"
                            rightText="Other" rightTextColor="#0079D3">
                        <InputField design="m2" defaults={this.state.inputFieldDefaults} />
                    </Dialog>
                }
                {
                    this.state.showReportDialog &&
                    <ReportDialog onClose={() => this.setState({ showReportDialog: false })} onNotifcation={this.props.onNotifcation} />
                }
               
            </div>
        );
    }
}

export default Home;