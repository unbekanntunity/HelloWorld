import React, { Component, createRef } from 'react';

import InputField from '../components/InputField/InputField';
import LeftBanner from '../components/LeftBanner';
import Project from '../components/Project';
import SpeedDial from '../components/SpeedDial';
import { Button, RoundButton } from '../components/Button';
import { Dialog, ReportDialog } from '../components/Dialog';
import MultiInputField from '../components/MultiInputField';

import menuOpened from '../images/close.png';
import menuClosed from '../images/dots-vertical.png';
import add from '../images/plus.png';
import startPreview from '../images/eye-open.png';
import stopPreview from '../images/eye-close.png';
import title from '../images/title.png';

import Ex1 from '../images/Progress1.png';
import Ex2 from '../images/Progress2.png';
import Ex3 from '../images/Progress3.png';

import ImageSection from '../components/ImageSection';
import LinkSection from '../components/LinkSection';
import { sendFORMRequest } from '../requestFuncs';
import TagSection from '../components/TagSection';

import './Projects.css'

class Projects extends Component {
    state = {
        projects: [
            {
                title: "HelloWorld",
                tags: [
                    {
                        name: "C#"
                    },
                    {
                        name: "MAUI"
                    }                ],
                description: "A social media app, including frontend and backend for mulitple plattforms. Its only for learning prurpose.",
                images: [Ex1, Ex2, Ex3],
                createdAt: "25.04",
                creatorImage: this.props.user.imageUrl,
                members: 4
            }
        ],
        showReportDialog: false,
        showStopPreviewButton: false,
        showCreateProjectDialog: false,
        existsCreateProjectDialog: true,

        currentName: "",
        currentDescription: ""
    }

    constructor(props) {
        super(props);

        this.imageSectionRef = createRef();
        this.tagSectionRef = createRef();
        this.linkSectionRef = createRef();
    }

    handleCreateProject = () => {
        this.setState({
            showCreateProjectDialog: true,
            existsCreateProjectDialog: true
        })
    }

    handleSubmit = () => {
        let formData = new FormData();

        formData.append("title", this.state.currentName);
        formData.append("description", this.state.currentDescription);

        let tagNames = this.tagSectionRef.current.getTags();
        for (var i = 0; i < tagNames.length; i++) {
            formData.append("tagNames", tagNames[i]);
        }

        let rawImages = this.imageSectionRef.current.getImages();
        for (var i = 0; i < rawImages.length; i++) {
            formData.append("rawImages", rawImages[i]);
        }

        let links = this.linkSectionRef.current.getLinks();
        for (var i = 0; i < links.length; i++) {
            formData.append("links", links[i]);
        }

        sendFORMRequest("POST", "/project/create", formData, this.props.tokens.token).then(
            response => {
                console.log(response);
                this.setState({ projects: [...this.state.projects, response.data] });
            },
            error => {
                console.log(error);
                this.props.onError(error.message);
            });
    }

    handleStartPreview = () => {
        let previewProject = {
            title: this.state.currentName,
            description: this.state.currentDescription,
            tagName: this.tagSectionRef.current.getTags(),
            rawImages: this.imageSectionRef.current.getImages(),
            links: this.linkSectionRef.current.getLinks(),
            creatorImage: this.props.user.image
        }

        if (!this.validateProject()) {
            return;
        }

        this.setState({
            showStopPreviewButton: true,
            showCreateProjectDialog: false,

            savedProjects: [...this.state.projects],
            projects: [previewProject]
        })

        setTimeout(() => console.log(this.state), 1000);
    }

    handleStopPreview = () => {
        this.setState({
            showStopPreviewButton: false,
            showCreateProjectDialog: true,

            savedProjects: [],
            projects: [...this.state.savedProjects]
        })
    }

    validateProject = () => {
        let result = this.state.currentName.length !== 0 && this.state.currentDescription.length !== 0

        if (!result) {
            this.props.onError("You can not let the content and the image section empty");
        }

        return result
    }

    render() {
        return (
            <div className="page-body flex">
                <div>
                    <LeftBanner text="Projects" />
                </div>
                <div className="projects-projects">
                    {
                    this.state.projects.map((item, index) =>
                        <Project key={index} title={item.title} createdAt={item.createdAt} description={item.description}
                            images={item.images} creatorImage={item.creatorImage} tags={item.tags} width={600} imageHeight={300} imageWidth={500}
                            onReportClick={() => this.setState({ showReportDialog: true })} />
                        )}
                </div>
                <div className="actionMenu">
                    {
                        !this.state.showStopPreviewButton &&
                        <SpeedDial radius={60} iconSize={30} itemFactor={.75}
                            menuOpenedIcon={menuOpened} menuClosedIcon={menuClosed} >
                            <SpeedDial.Item icon={add} onClick={this.handleCreateProject} />
                        </SpeedDial>
                    }
                    {
                        this.state.showStopPreviewButton &&
                        <RoundButton icon={stopPreview} radius={60} iconSize={30} onClick={this.handleStopPreview} />
                    }
                </div>
                {
                    this.state.existsCreateProjectDialog &&
                    <div className="dialog-container" style={{ display: this.state.showCreateProjectDialog ? 'inherit' : 'none' }} >
                        <Dialog title="Create new project" height="fit-content" width={500 } paddingX="20px" paddingY="20px"
                            onBackClick={() => this.setState({
                            showCreateProjectDialog: false,
                            existsCreateProjectDialog: false,
                            })} rightIcon={startPreview} onRightClick={this.handleStartPreview} backButton={true}>
                            <InputField icon={title} iconSize={20} placeholder="Project name" width={300}
                                    onChange={(event) => this.setState({ currentName: event.target.value })} />
                            <div className="create-multi-container">
                                <p className="discussions-multi-label">Description</p>
                                <MultiInputField placeholder="" height="200px" maxLetters={300}
                                    onChange={(event) => this.setState({ currentDescription: event.target.value })} zindex={1} />
                                </div>
                            <TagSection ref={this.tagSectionRef} tokens={this.props.tokens} onError={this.props.onError} zIndex={2} tagType="Projects" />
                            <ImageSection ref={this.imageSectionRef} imageSize={40} />
                            <LinkSection ref={this.linkSectionRef} />
                            <div className="center">
                                <Button text="Create" onClick={this.handleSubmit} />
                            </div>
                        </Dialog>
                    </div>
                }
                {
                    this.state.showReportDialog &&
                    <ReportDialog onClose={() => this.setState({ showReportDialog: false })} onNotification={this.props.onNotification} />
                }
            </div>
        )
    }
}

export default Projects;