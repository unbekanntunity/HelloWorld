import React, { Component } from 'react';

import arrowLeft from '../images/left-arrow.png';
import arrowRight from '../images/right-arrow.png';

import './ImageSlider.css';

class ImageSlider extends Component {
    state = {
        amountDots: this.props.images?.length ?? 0,
        currentDot: 0,

        currentImage: this.props.images[0],
    }

    handleSliderDotClick = (element) => {
        let dot = null;

        if (element.className !== "circle" && element.className !== "circle active") {
            dot = element.lastChild;
        }
        else {
            dot = element;
        }

        let dots = Array.from(dot.parentElement.parentElement.childNodes).filter(x => x.className !=="img-slider-image");
        dots.forEach(a => a.lastChild.className = "circle");

        dot.className = "circle active";
        this.setState({
            currentDot: parseInt(dot.parentElement.id),
            currentImage: this.props.images[parseInt(dot.parentElement.id)]
        });
    }

    handlePicButtonPressed = (direction) => {
        let updatedIndex = 0;

        if (this.state.currentDot + direction === -1) {
            updatedIndex = this.state.amountDots - 1;
        }
        else if (this.state.currentDot + direction === this.state.amountDots) {
            updatedIndex = 0;
        }
        else {
            updatedIndex = this.state.currentDot + direction;
        }

        this.handleSliderDotClick(document.getElementById(updatedIndex.toString()));

        this.setState({
            currentImage: this.props.images[updatedIndex]
        })
    }

    render() {
        return (
            <div className="img-slider-container">
                {
                    this.props.images?.length !== 0 &&
                    <a href={this.state.backLink}>
                        <img id="pic-btn-back" className="post-pic-next" src={arrowLeft} height={20} width={20} alt=""
                            onClick={() => this.handlePicButtonPressed(-1)} />
                    </a>
                }
                <div className="img-slider-slider" style={{ width: this.props.imageWidth }}>
                    <div className="img-slider-image">
                        <img src={this.state.currentImage} alt="" height={this.props.imageHeight} />
                    </div>
                    {
                        this.props.images.map((_, index) =>
                            <a id={index} key={index} onClick={(event) => this.handleSliderDotClick(event.target, event)}>
                                <div className={index === 0 ? "circle active" : "circle"} />
                            </a>
                    )
                    }
                </div>
                {
                    this.props.images?.length !== 0 &&
                    <a href={this.state.forwardLink}>
                        <img id="pic-btn-forward" className="post-pic-next" src={arrowRight} height={20} width={20} alt=""
                            onClick={() => this.handlePicButtonPressed(1)} />
                    </a>
                }
            </div>    
        )
    }
}

export default ImageSlider;