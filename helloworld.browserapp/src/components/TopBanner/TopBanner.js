import React, { Children, Component } from 'react';
import SimpleItem from './SimpleItem';

import './TopBanner.css';
import UnderlineItem from './UnderlineItem';

class TopBanner extends Component {
    state = {
        selectedIndex: 0
    }

    static SimpleItem = SimpleItem;
    static UnderlineItem = UnderlineItem;

    handleCategoryChange = (index) => {
        if (index === this.state.selectedIndex) {
            return;
        }

        this.setState({
            selectedIndex: index
        })

        this.props.onSelectionChanged(index);
    }

    renderItem = (item, index) => {
        if (this.props.design === undefined || this.props.design === "M1") {
            return <p className="top-banner-item" style={{
                color: this.state.selectedIndex === index ? this.props.selectedTextColor : this.props.unselectedTextColor,
            }}
                onClick={() => this.handleCategoryChange(index)}>{item}</p>
        }
        else {
            return <p className="top-banner-item" style={{ color: this.state.selectedIndex === index ? this.props.selectedTextColoe : this.props.unselectedTextColor }}
                onClick={() => this.handleCategoryChange(index)}>{item}</p>
        }
    }

    render() {
        return (
            <div className="top-banner-container" style={{
                backgroundColor: this.props.bgColor
            }}>
                {
                    Children.map(this.props.children, (child, index) => 
                        React.cloneElement(child, {
                            className: "top-banner-item",
                            key: index,
                            keyProp: index,
                            onClick: this.handleCategoryChange,
                            selected: this.state.selectedIndex === index
                    }))
                }
            </div>   
        )
    }
}

export default TopBanner;